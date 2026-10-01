namespace Shared.Extensions;

public static class GuidV7Extensions
{
    public static DateTime GetCreatedAtUtc(this Guid guid)
    {
        Span<byte> bytes = stackalloc byte[16];

        guid.TryWriteBytes(bytes, bigEndian: true, out _);

        if ((bytes[6] >> 4) != 7)
            throw new ArgumentException("Указанный GUID не является версией 7 (UUIDv7)", nameof(guid));

        long timestampMs = ((long)bytes[0] << 40) | ((long)bytes[1] << 32) | ((long)bytes[2] << 24) | ((long)bytes[3] << 16) | ((long)bytes[4] << 8) | bytes[5];

        return DateTimeOffset.FromUnixTimeMilliseconds(timestampMs).UtcDateTime;
    }
}
