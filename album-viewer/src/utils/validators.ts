/**
 * Validates and converts a date string in French format (DD/MM/YYYY) to a Date object.
 * @param dateString - The date string to validate in format DD/MM/YYYY
 * @returns A Date object if valid, null otherwise
 */
export function validateDate(dateString: string): Date | null {
    if (!dateString || typeof dateString !== 'string') {
        return null;
    }

    const trimmed = dateString.trim();
    const datePattern = /^(\d{2})\/(\d{2})\/(\d{4})$/;
    const match = trimmed.match(datePattern);

    if (!match) {
        return null;
    }

    const day = parseInt(match[1]!, 10);
    const month = parseInt(match[2]!, 10);
    const year = parseInt(match[3]!, 10);

    // Validate month range
    if (month < 1 || month > 12) {
        return null;
    }

    // Validate day range
    if (day < 1 || day > 31) {
        return null;
    }

    // Create date object (month is 0-indexed in JavaScript)
    const date = new Date(year, month - 1, day);

    // Verify the date is valid (handles invalid dates like 31/02/2024)
    if (
        date.getDate() !== day ||
        date.getMonth() !== month - 1 ||
        date.getFullYear() !== year
    ) {
        return null;
    }

    return date;
}

/**
 * Validates the format of a GUID string.
 * @param guid - The GUID string to validate
 * @returns true if the string is a valid GUID format, false otherwise
 */
export function validateGuid(guid: string): boolean {
    if (!guid || typeof guid !== 'string') {
        return false;
    }

    const guidPattern = /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i;
    return guidPattern.test(guid.trim());
}

/**
 * Validates the format of an IPv6 address string.
 * @param ipv6 - The IPv6 address string to validate
 * @returns true if the string is a valid IPv6 format, false otherwise
 */
export function validateIPV6(ipv6: string): boolean {
    if (!ipv6 || typeof ipv6 !== 'string') {
        return false;
    }

    const trimmed = ipv6.trim();
    
    // IPv6 pattern supports full notation, compressed notation (::), and mixed notation
    const ipv6Pattern = /^(([0-9a-f]{1,4}:){7}[0-9a-f]{1,4}|([0-9a-f]{1,4}:){1,7}:|([0-9a-f]{1,4}:){1,6}:[0-9a-f]{1,4}|([0-9a-f]{1,4}:){1,5}(:[0-9a-f]{1,4}){1,2}|([0-9a-f]{1,4}:){1,4}(:[0-9a-f]{1,4}){1,3}|([0-9a-f]{1,4}:){1,3}(:[0-9a-f]{1,4}){1,4}|([0-9a-f]{1,4}:){1,2}(:[0-9a-f]{1,4}){1,5}|[0-9a-f]{1,4}:((:[0-9a-f]{1,4}){1,6})|:((:[0-9a-f]{1,4}){1,7}|:))$/i;
    
    return ipv6Pattern.test(trimmed);
}