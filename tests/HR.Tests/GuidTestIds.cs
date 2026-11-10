global using static HR.Tests.GuidTestIds;

namespace HR.Tests;

public static class GuidTestIds
{
    // Deterministic UUIDs keep fixtures readable without introducing numeric business IDs.
    public static Guid Id(int seed) => new(seed, 0, 0, new byte[8]);
}
