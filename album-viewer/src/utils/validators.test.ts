import { describe, it, expect } from 'vitest';
import { validateDate, validateGuid, validateIPV6 } from './validators';

describe('validateDate', () => {
    it('should return a valid Date object for valid French date format', () => {
        const result = validateDate('15/06/2024');
        expect(result).toBeInstanceOf(Date);
        expect(result?.getDate()).toBe(15);
        expect(result?.getMonth()).toBe(5); // 0-indexed
        expect(result?.getFullYear()).toBe(2024);
    });

    it('should return a valid Date object for edge date cases', () => {
        const leapYear = validateDate('29/02/2024');
        expect(leapYear).toBeInstanceOf(Date);
        expect(leapYear?.getDate()).toBe(29);

        const endOfYear = validateDate('31/12/2023');
        expect(endOfYear).toBeInstanceOf(Date);

        const startOfYear = validateDate('01/01/2000');
        expect(startOfYear).toBeInstanceOf(Date);
    });

    it('should return null for invalid date format', () => {
        expect(validateDate('2024-06-15')).toBeNull(); // Wrong format
        expect(validateDate('15-06-2024')).toBeNull(); // Wrong separator
        expect(validateDate('15/6/2024')).toBeNull(); // Single digit month
        expect(validateDate('5/06/2024')).toBeNull(); // Single digit day
        expect(validateDate('15/06/24')).toBeNull(); // Two-digit year
    });

    it('should return null for invalid dates', () => {
        expect(validateDate('31/02/2024')).toBeNull(); // Invalid day for February
        expect(validateDate('29/02/2023')).toBeNull(); // Not a leap year
        expect(validateDate('32/01/2024')).toBeNull(); // Day out of range
        expect(validateDate('15/13/2024')).toBeNull(); // Month out of range
        expect(validateDate('00/06/2024')).toBeNull(); // Day zero
        expect(validateDate('15/00/2024')).toBeNull(); // Month zero
    });

    it('should return null for empty or invalid input', () => {
        expect(validateDate('')).toBeNull();
        expect(validateDate('   ')).toBeNull();
        expect(validateDate('not-a-date')).toBeNull();
    });

    it('should handle whitespace by trimming', () => {
        const result = validateDate('  15/06/2024  ');
        expect(result).toBeInstanceOf(Date);
        expect(result?.getDate()).toBe(15);
    });
});

describe('validateGuid', () => {
    it('should return true for valid GUID formats', () => {
        expect(validateGuid('550e8400-e29b-41d4-a716-446655440000')).toBe(true);
        expect(validateGuid('123e4567-e89b-12d3-a456-426614174000')).toBe(true);
        expect(validateGuid('00000000-0000-0000-0000-000000000000')).toBe(true);
    });

    it('should be case-insensitive', () => {
        expect(validateGuid('550E8400-E29B-41D4-A716-446655440000')).toBe(true);
        expect(validateGuid('550e8400-E29B-41d4-A716-446655440000')).toBe(true);
    });

    it('should return false for invalid GUID formats', () => {
        expect(validateGuid('550e8400-e29b-41d4-a716')).toBe(false); // Too short
        expect(validateGuid('550e8400-e29b-41d4-a716-446655440000-extra')).toBe(false); // Too long
        expect(validateGuid('550e8400e29b41d4a716446655440000')).toBe(false); // Missing dashes
        expect(validateGuid('550e8400-e29b-41d4-a716-44665544000g')).toBe(false); // Invalid character 'g'
        expect(validateGuid('550e8400-e29b-41d4-a71-6446655440000')).toBe(false); // Wrong segment length
    });

    it('should return false for empty or invalid input', () => {
        expect(validateGuid('')).toBe(false);
        expect(validateGuid('   ')).toBe(false);
        expect(validateGuid('not-a-guid')).toBe(false);
    });

    it('should handle whitespace by trimming', () => {
        expect(validateGuid('  550e8400-e29b-41d4-a716-446655440000  ')).toBe(true);
    });
});

describe('validateIPV6', () => {
    it('should return true for valid full IPv6 addresses', () => {
        expect(validateIPV6('2001:0db8:85a3:0000:0000:8a2e:0370:7334')).toBe(true);
        expect(validateIPV6('2001:db8:85a3:0:0:8a2e:370:7334')).toBe(true);
        expect(validateIPV6('0000:0000:0000:0000:0000:0000:0000:0001')).toBe(true);
    });

    it('should return true for compressed IPv6 addresses', () => {
        expect(validateIPV6('2001:db8::8a2e:370:7334')).toBe(true);
        expect(validateIPV6('::1')).toBe(true); // Loopback
        expect(validateIPV6('::ffff:192.0.2.1')).toBe(false); // Mixed notation with IPv4 (not supported by this pattern)
        expect(validateIPV6('2001:db8::')).toBe(true);
        expect(validateIPV6('::2001:db8')).toBe(true);
    });

    it('should return true for various valid IPv6 notations', () => {
        expect(validateIPV6('fe80::1')).toBe(true);
        expect(validateIPV6('ff02::1')).toBe(true);
        expect(validateIPV6('2001:0db8:0001:0000:0000:0ab9:C0A8:0102')).toBe(true);
    });

    it('should be case-insensitive', () => {
        expect(validateIPV6('2001:0DB8:85A3:0000:0000:8A2E:0370:7334')).toBe(true);
        expect(validateIPV6('2001:db8:85a3:0:0:8a2e:370:7334')).toBe(true);
    });

    it('should return false for invalid IPv6 addresses', () => {
        expect(validateIPV6('2001:0db8:85a3::8a2e:370g:7334')).toBe(false); // Invalid character 'g'
        expect(validateIPV6('2001:0db8:85a3::8a2e::7334')).toBe(false); // Double ::
        expect(validateIPV6('02001:0db8:0000:0000:0000:0000:0000:0000')).toBe(false); // Segment too long
        expect(validateIPV6('2001:db8')).toBe(false); // Too short without compression
        expect(validateIPV6('gggg::1')).toBe(false); // Invalid characters
    });

    it('should return false for empty or invalid input', () => {
        expect(validateIPV6('')).toBe(false);
        expect(validateIPV6('   ')).toBe(false);
        expect(validateIPV6('not-an-ipv6')).toBe(false);
        expect(validateIPV6('192.168.1.1')).toBe(false); // IPv4
    });

    it('should handle whitespace by trimming', () => {
        expect(validateIPV6('  2001:db8::1  ')).toBe(true);
    });
});
