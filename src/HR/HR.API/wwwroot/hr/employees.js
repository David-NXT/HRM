const api = '/employees';
const statusBox = document.querySelector('#status'), rows = document.querySelector('#rows');
const dialog = document.querySelector('#editor'), form = document.querySelector('#employeeForm');
const salaryDialog = document.querySelector('#salaryEditor'), salaryForm = document.querySelector('#salaryForm');
const detailDialog = document.querySelector('#employeeDetails');
const numberFormat = new Intl.NumberFormat('vi-VN');
let editingId = null, salaryEmployeeId = null, page = 1, total = 0, appliedKeyword = '', loadVersion = 0;
const pageSize = 20;

async function request(url, options = {}) {
    const response = await fetch(url, { ...options, headers: { 'Content-Type': 'application/json', 'X-Role': 'HR', ...(options.headers || {}) } });
    const data = await response.json().catch(() => null);
    if (!response.ok) throw Error(data?.message || `Yêu cầu không thành công (HTTP ${response.status}).`);
    return data;
}
function feedback(message = '', state = '') {
    statusBox.textContent = message;
    statusBox.dataset.state = state;
}
function text(tag, value, className = '') {
    const node = document.createElement(tag);
    node.textContent = value ?? '—';
    if (className) node.className = className;
    return node;
}
function svg(name) {
    const node = document.createElementNS('http://www.w3.org/2000/svg', 'svg');
    node.setAttribute('class', 'icon'); node.setAttribute('aria-hidden', 'true');
    const use = document.createElementNS('http://www.w3.org/2000/svg', 'use');
    use.setAttribute('href', '#i-' + name); node.append(use); return node;
}
function action(name, label, handler, danger = false) {
    const button = document.createElement('button');
    button.type = 'button'; button.className = 'icon-button' + (danger ? ' danger' : '');
    button.title = label; button.setAttribute('aria-label', label); button.append(svg(name));
    button.onclick = handler; return button;
}
function salaryValue(employee) { return employee.employmentType === 'PART_TIME' ? employee.partTimeSalary : employee.baseSalary; }
function salaryUnit(employee) { return employee.employmentType === 'PART_TIME' ? 'VND/giờ' : 'VND/tháng'; }
function dateValue(value) { return value ? new Date(value + 'T00:00:00').toLocaleDateString('vi-VN') : '—'; }

async function loadStatistics() {
    try {
        const data = await request(api + '/statistics');
        for (const [id, key] of [['total', 'totalEmployees'], ['active', 'activeEmployees'], ['full', 'fullTimeEmployees'], ['part', 'partTimeEmployees']])
            document.querySelector('#stat-' + id).textContent = numberFormat.format(data[key]);
        document.querySelector('#stat-status-note').textContent = `${data.lockedEmployees} đã khóa · ${data.resignedEmployees} đã nghỉ việc`;
    } catch {
        for (const id of ['total', 'active', 'full', 'part']) document.querySelector('#stat-' + id).textContent = '—';
        document.querySelector('#stat-status-note').textContent = 'Chưa tải được thống kê';
    }
}
function renderEmployee(employee, index) {
    const row = document.createElement('tr');
    const identity = document.createElement('td'), person = document.createElement('div'); person.className = 'employee-cell';
    const initials = employee.fullName.trim().split(/\s+/).slice(-2).map(word => word[0]).join('').toUpperCase();
    const avatar = text('span', initials, 'employee-avatar'); avatar.dataset.color = String(index % 4); avatar.setAttribute('aria-hidden', 'true');
    const caption = document.createElement('div'), name = text('button', employee.fullName, 'employee-name'); name.type = 'button'; name.onclick = () => showDetails(employee);
    caption.append(name, text('span', employee.employeeCode, 'cell-secondary employee-code'), text('span', hrLabel(employee.employmentType), 'cell-secondary'));
    person.append(avatar, caption); identity.append(person); row.append(identity);
    const contact = document.createElement('td'); contact.append(text('span', employee.email), text('span', employee.phone, 'cell-secondary')); row.append(contact);
    const work = document.createElement('td'); work.append(text('span', employee.departmentName || 'Chưa phân công'), text('span', employee.positionName || 'Chưa có chức vụ', 'cell-secondary')); row.append(work);
    const salary = document.createElement('td'), amount = salaryValue(employee);
    salary.append(text('span', amount == null ? 'Chưa cập nhật' : numberFormat.format(amount), 'salary-value'), text('span', salaryUnit(employee), 'cell-secondary')); row.append(salary);
    row.append(text('td', hrLabel(employee.status)));
    const actionsCell = document.createElement('td'), actions = document.createElement('div'); actions.className = 'actions';
    actions.append(action('eye', 'Xem chi tiết ' + employee.employeeCode, () => showDetails(employee)), action('edit', 'Sửa hồ sơ ' + employee.employeeCode, () => openEditor(employee)), action('money', 'Cập nhật lương ' + employee.employeeCode, () => openSalary(employee)));
    if (employee.status !== 'LOCKED' && employee.status !== 'RESIGNED') actions.append(action('lock', 'Khóa hồ sơ ' + employee.employeeCode, async () => {
        if (!confirm('Khóa hồ sơ ' + employee.employeeCode + '?')) return;
        try { await request(api + '/' + employee.employeeId + '/lock', { method: 'POST' }); await load(); } catch (error) { feedback(hrErrorMessage(error), 'error'); }
    }));
    actions.append(action('trash', 'Xóa hồ sơ ' + employee.employeeCode, async () => {
        if (!confirm('Xóa vĩnh viễn hồ sơ ' + employee.employeeCode + '? Chỉ hồ sơ chưa phát sinh dữ liệu nghiệp vụ mới xóa được.')) return;
        try { await request(api + '/' + employee.employeeId, { method: 'DELETE' }); await load(); } catch (error) { feedback(hrErrorMessage(error), 'error'); }
    }, true));
    actionsCell.append(actions); row.append(actionsCell);
    row.ondblclick = event => { if (!event.target.closest('button')) showDetails(employee); };
    return row;
}
async function load() {
    const version = ++loadVersion;
    feedback('Đang tải danh sách nhân viên…', 'loading');
    document.querySelector('#previous').disabled = true; document.querySelector('#next').disabled = true;
    try {
        const query = new URLSearchParams({ page: String(page), pageSize: String(pageSize), keyword: appliedKeyword });
        const data = await request(api + '?' + query);
        if (version !== loadVersion) return;
        total = data.total;
        if (!data.items.length && page > 1) { page = Math.max(1, Math.ceil(total / pageSize)); return load(); }
        rows.replaceChildren(...data.items.map(renderEmployee));
        if (!data.items.length) {
            const row = document.createElement('tr'), cell = text('td', 'Không tìm thấy nhân viên phù hợp. Thử một từ khóa khác.', 'empty-cell');
            cell.colSpan = 6; row.append(cell); rows.append(row);
        }
        document.querySelector('#employee-count').textContent = numberFormat.format(total) + ' hồ sơ';
        const start = total ? (page - 1) * pageSize + 1 : 0, end = Math.min(page * pageSize, total);
        document.querySelector('#pageInfo').textContent = `Hiển thị ${start}–${end} trong ${numberFormat.format(total)} nhân viên` + (appliedKeyword ? ' phù hợp' : '');
        document.querySelector('#pageNumber').textContent = page;
        document.querySelector('#previous').disabled = page <= 1;
        document.querySelector('#next').disabled = page * pageSize >= total;
        feedback();
    } catch (error) { if (version === loadVersion) feedback(hrErrorMessage(error), 'error'); }
    loadStatistics();
}
function showDetails(employee) {
    document.querySelector('#detailTitle').textContent = employee.fullName;
    document.querySelector('#detailSubtitle').textContent = employee.employeeCode + ' · ' + hrLabel(employee.employmentType);
    for (const field of detailDialog.querySelectorAll('[data-detail]')) {
        const key = field.dataset.detail;
        let value = employee[key];
        if (key === 'dateOfBirth') value = dateValue(value);
        else if (['gender', 'status', 'employmentType'].includes(key)) value = hrLabel(value);
        else if (key === 'salary') value = salaryValue(employee) == null ? 'Chưa cập nhật' : numberFormat.format(salaryValue(employee)) + ' ' + salaryUnit(employee);
        field.textContent = value || 'Chưa cập nhật';
    }
    detailDialog.showModal();
}
function updateSalaryFields() {
    const partTime = form.elements.employmentType.value === 'PART_TIME';
    document.querySelector('#fullSalaryField').hidden = partTime;
    document.querySelector('#partSalaryField').hidden = !partTime;
    form.elements.baseSalary.disabled = !!editingId || partTime;
    form.elements.partTimeSalary.disabled = !!editingId || !partTime;
}
function openEditor(employee = null) {
    editingId = employee?.employeeId ?? null;
    form.reset(); document.querySelector('#formError').textContent = '';
    form.elements.employeeCode.disabled = !!employee;
    form.elements.employmentType.disabled = !!employee;
    form.elements.employmentType.value = employee?.employmentType ?? 'FULL_TIME';
    form.elements.baseSalary.value = employee?.baseSalary ?? 0;
    form.elements.partTimeSalary.value = employee?.partTimeSalary ?? 0;
    document.querySelector('#formTitle').textContent = employee ? 'Sửa hồ sơ nhân viên' : 'Thêm nhân viên';
    if (employee) {
        for (const key of ['employeeCode', 'fullName', 'gender', 'email', 'phone', 'citizenId', 'address']) form.elements[key].value = employee[key] ?? '';
        form.elements.dateOfBirth.value = employee.dateOfBirth;
    }
    updateSalaryFields(); dialog.showModal();
}
function openSalary(employee) {
    salaryEmployeeId = employee.employeeId;
    const partTime = employee.employmentType === 'PART_TIME';
    salaryForm.elements.salaryAmount.value = salaryValue(employee) ?? 0;
    document.querySelector('#salaryError').textContent = '';
    document.querySelector('#salaryTitle').textContent = partTime ? 'Cập nhật đơn giá bán thời gian' : 'Cập nhật lương cơ bản';
    document.querySelector('#salaryLabel').firstChild.textContent = partTime ? 'Đơn giá (VND/giờ)' : 'Lương cơ bản (VND/tháng)';
    salaryDialog.dataset.salaryProperty = partTime ? 'partTimeSalary' : 'baseSalary';
    salaryDialog.showModal();
}
document.querySelector('#add').onclick = () => openEditor();
document.querySelector('#search').onclick = () => { appliedKeyword = document.querySelector('#keyword').value.trim(); page = 1; load(); };
document.querySelector('#keyword').onkeydown = event => { if (event.key === 'Enter') document.querySelector('#search').click(); };
document.querySelector('#previous').onclick = () => { if (page > 1) { page--; load(); } };
document.querySelector('#next').onclick = () => { if (page * pageSize < total) { page++; load(); } };
document.querySelector('#cancel').onclick = () => dialog.close();
document.querySelector('#cancelSalary').onclick = () => salaryDialog.close();
document.querySelector('#closeDetails').onclick = document.querySelector('#doneDetails').onclick = () => detailDialog.close();
form.elements.employmentType.onchange = updateSalaryFields;
form.onsubmit = async event => {
    event.preventDefault();
    const value = Object.fromEntries(new FormData(form)); value.address = value.address.trim() || null;
    if (editingId) { delete value.baseSalary; delete value.partTimeSalary; delete value.employeeCode; }
    else if (value.employmentType === 'PART_TIME') { value.partTimeSalary = Number(value.partTimeSalary); delete value.baseSalary; }
    else { value.baseSalary = Number(value.baseSalary); delete value.partTimeSalary; }
    const submit = form.querySelector('[type=submit]'); submit.disabled = true;
    try {
        await request(editingId ? api + '/' + editingId : api, { method: editingId ? 'PUT' : 'POST', body: JSON.stringify(value) });
        dialog.close(); await load();
    } catch (error) { const target = document.querySelector('#formError'); target.textContent = hrErrorMessage(error); target.dataset.state = 'error'; }
    finally { submit.disabled = false; }
};
salaryForm.onsubmit = async event => {
    event.preventDefault();
    const submit = salaryForm.querySelector('[type=submit]'); submit.disabled = true;
    try {
        await request(api + '/' + salaryEmployeeId + '/salary', { method: 'PUT', body: JSON.stringify({ [salaryDialog.dataset.salaryProperty]: Number(salaryForm.elements.salaryAmount.value) }) });
        salaryDialog.close(); await load();
    } catch (error) { const target = document.querySelector('#salaryError'); target.textContent = hrErrorMessage(error); target.dataset.state = 'error'; }
    finally { submit.disabled = false; }
};
document.querySelector('#export').onclick = async () => {
    const button = document.querySelector('#export'); button.disabled = true;
    try {
        const response = await fetch(api + '/export?' + new URLSearchParams({ keyword: appliedKeyword }), { headers: { 'X-Role': 'HR' } });
        if (!response.ok) { const error = await response.json().catch(() => null); throw Error(error?.message || 'Không thể xuất danh sách nhân viên.'); }
        const url = URL.createObjectURL(await response.blob()), link = document.createElement('a');
        link.href = url; link.download = 'DanhSachNhanVien.xlsx'; document.body.append(link); link.click(); link.remove();
        setTimeout(() => URL.revokeObjectURL(url), 1000);
        feedback('Đã tải file Excel theo bộ lọc đang áp dụng.');
    } catch (error) { feedback(hrErrorMessage(error), 'error'); }
    finally { button.disabled = false; }
};
load();
