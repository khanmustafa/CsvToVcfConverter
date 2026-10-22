/* Pure conversion helpers shared by the browser and Node tests. */
(function (root) {
  'use strict';
  const text = value => String(value ?? '').trim();
  function contactsFromRows(rows, nameIndex, phoneIndex) {
    if (nameIndex < 0 || phoneIndex < 0 || nameIndex === phoneIndex) return [];
    return rows.map(row => ({ name: text(row[nameIndex]), phone: text(row[phoneIndex]) }))
      .filter(contact => contact.name && contact.phone);
  }
  function escapeText(value) {
    return text(value).replace(/\\/g, '\\\\').replace(/\r\n|\r|\n/g, '\\n').replace(/;/g, '\\;').replace(/,/g, '\\,');
  }
  function foldLine(line) {
    const encoder = new TextEncoder();
    let result = '', length = 0;
    for (const char of line) {
      const bytes = encoder.encode(char).length;
      if (length + bytes > 75) { result += '\r\n '; length = 1; }
      result += char; length += bytes;
    }
    return result;
  }
  function toVcf(contacts) {
    return contacts.map(({ name, phone }) => [
      'BEGIN:VCARD', 'VERSION:3.0', `FN:${escapeText(name)}`, `N:${escapeText(name)};;;;`,
      `TEL;TYPE=CELL:${escapeText(phone)}`, 'END:VCARD'
    ].map(foldLine).join('\r\n')).join('\r\n') + '\r\n';
  }
  const api = { contactsFromRows, escapeText, foldLine, toVcf };
  if (typeof module !== 'undefined' && module.exports) module.exports = api;
  else root.ContactConverter = api;
})(typeof globalThis !== 'undefined' ? globalThis : this);
