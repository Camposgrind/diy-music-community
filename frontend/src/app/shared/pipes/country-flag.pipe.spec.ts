import { CountryFlagPipe } from './country-flag.pipe';

describe('CountryFlagPipe', () => {
  let pipe: CountryFlagPipe;

  beforeEach(() => (pipe = new CountryFlagPipe()));

  it('should return the lowercase ISO-2 code for a known country', () => {
    expect(pipe.transform('United States')).toBe('us');
    expect(pipe.transform('Germany')).toBe('de');
    expect(pipe.transform('Brazil')).toBe('br');
    expect(pipe.transform('Spain')).toBe('es');
    expect(pipe.transform('United Kingdom')).toBe('gb');
  });

  it('should return an empty string for an unknown country', () => {
    expect(pipe.transform('Nowhereland')).toBe('');
    expect(pipe.transform('')).toBe('');
  });

  it('should be case-sensitive (exact match required)', () => {
    expect(pipe.transform('germany')).toBe('');
    expect(pipe.transform('BRAZIL')).toBe('');
  });

  it('should handle countries with spaces in their name', () => {
    expect(pipe.transform('Bosnia and Herzegovina')).toBe('ba');
    expect(pipe.transform('New Zealand')).toBe('nz');
    expect(pipe.transform('Czech Republic')).toBe('cz');
  });

  it('should always return a lowercase code', () => {
    const result = pipe.transform('France');
    expect(result).toBe(result.toLowerCase());
  });
});
