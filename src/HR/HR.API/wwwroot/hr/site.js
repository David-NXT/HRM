(() => {
    const toggle = document.querySelector('#nav-toggle');
    const overlay = document.querySelector('#nav-overlay');
    const sidebar = document.querySelector('#app-sidebar');
    const workspace = document.querySelector('.app-workspace');
    function setNavigation(open) {
        const mobile = window.innerWidth <= 900;
        open = mobile && open;
        document.body.classList.toggle('nav-open', open);
        toggle?.setAttribute('aria-expanded', String(open));
        if (overlay) overlay.hidden = !open;
        if (sidebar) sidebar.inert = mobile && !open;
        if (workspace) workspace.inert = open;
        if (open) sidebar?.querySelector('.is-active')?.focus();
    }
    toggle?.addEventListener('click', () => setNavigation(toggle.getAttribute('aria-expanded') !== 'true'));
    overlay?.addEventListener('click', () => { setNavigation(false); toggle?.focus(); });
    document.addEventListener('keydown', event => {
        if (event.key === 'Escape' && document.body.classList.contains('nav-open')) {
            setNavigation(false);
            toggle?.focus();
        }
    });
    window.addEventListener('resize', () => setNavigation(document.body.classList.contains('nav-open')));
    setNavigation(false);
    const date = document.querySelector('#today-label');
    if (date) date.textContent = new Intl.DateTimeFormat('vi-VN', { day: '2-digit', month: 'long', year: 'numeric' }).format(new Date());

    const tones = new Map([
        ['Đang hoạt động', 'green'], ['Đã chấp thuận', 'green'], ['Đã hoàn tất', 'green'],
        ['Chờ duyệt', 'amber'], ['Cần bổ sung thông tin', 'amber'], ['Đang xử lý', 'amber'],
        ['Đã khóa', 'amber'], ['Đã nghỉ việc', 'slate'], ['Ngừng hoạt động', 'slate'],
        ['Đã hủy', 'slate'], ['Bản nháp', 'slate'], ['Đã gia hạn', 'indigo'],
        ['Đã xem', 'indigo'], ['Mới', 'indigo'], ['Đã từ chối', 'red']
    ]);
    function enhanceTables() {
        document.querySelectorAll('.page-content table').forEach(table => {
            if (!table.closest('.table-scroll, .table')) {
                const wrapper = document.createElement('div');
                wrapper.className = 'table-scroll';
                table.before(wrapper);
                wrapper.append(table);
            }
            table.querySelectorAll('tbody').forEach(body => {
                if (!body.children.length) {
                    const row = body.insertRow();
                    row.className = 'empty-row';
                    const cell = row.insertCell();
                    cell.colSpan = table.querySelectorAll('thead th').length || 1;
                    cell.className = 'empty-cell';
                    cell.textContent = 'Chưa có dữ liệu để hiển thị.';
                }
            });
            table.querySelectorAll('td').forEach(cell => {
                const text = cell.textContent.trim();
                if (!tones.has(text) || cell.children.length) return;
                const badge = document.createElement('span');
                badge.className = 'badge';
                badge.dataset.tone = tones.get(text);
                badge.textContent = text;
                cell.replaceChildren(badge);
            });
        });
    }
    enhanceTables();
    const content = document.querySelector('#main-content');
    if (content) new MutationObserver(enhanceTables).observe(content, { childList: true, subtree: true });
})();
