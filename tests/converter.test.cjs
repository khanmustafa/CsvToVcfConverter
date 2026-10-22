const assert = require('node:assert/strict');
const test = require('node:test');
const XLSX = require('../public/vendor/xlsx.full.min.js');
const { contactsFromRows, toVcf, foldLine } = require('../public/converter.js');
test('CSV preserves plus signs, leading zeros, quoted names and multiline fields', () => {
 const book = XLSX.read('Name,Phone\r\n"Doe, Jane",+12345\r\n"Two\nLines",001234\r\nMissing,\r\n', {type:'string',raw:true});
 const rows = XLSX.utils.sheet_to_json(book.Sheets[book.SheetNames[0]],{header:1,raw:false,defval:'',blankrows:false});
 assert.deepEqual(contactsFromRows(rows.slice(1),0,1), [{name:'Doe, Jane',phone:'+12345'},{name:'Two\nLines',phone:'001234'}]);
});
test('Excel formats and worksheets preserve text phone numbers', () => {
 for(const bookType of ['xlsx','biff8']) {
  const book = XLSX.utils.book_new();
  XLSX.utils.book_append_sheet(book,XLSX.utils.aoa_to_sheet([['Name','Phone'],['Alex','001234'],['Priya','+919876543210']]),'Contacts');
  XLSX.utils.book_append_sheet(book,XLSX.utils.aoa_to_sheet([['Name','Phone']]),'Empty');
  const loaded = XLSX.read(XLSX.write(book,{type:'buffer',bookType}),{type:'buffer'});
  const rows = XLSX.utils.sheet_to_json(loaded.Sheets.Contacts,{header:1,raw:false,defval:''});
  assert.equal(loaded.SheetNames.length,2);
  assert.equal(contactsFromRows(rows.slice(1),0,1)[0].phone,'001234');
 }
});
test('VCF escapes untrusted fields and uses CRLF', () => {
 const vcf = toVcf([{name:'Doe, Jane; Jr\\\nNext',phone:'+123\nEND:VCARD'}]);
 assert.ok(vcf.includes('FN:Doe\\, Jane\\; Jr\\\\\\nNext\r\n'));
 assert.ok(vcf.includes('TEL;TYPE=CELL:+123\\nEND:VCARD\r\n'));
 assert.equal(vcf.split('BEGIN:VCARD').length,2);
 assert.equal(vcf.replace(/\r\n/g,'').includes('\n'),false);
});
test('Unicode lines fold at 75 UTF-8 bytes without splitting characters', () => {
 const input='FN:'+'联系人🙂'.repeat(40);
 const folded=foldLine(input);
 assert.equal(folded.replace(/\r\n /g,''),input);
 for(const line of folded.split('\r\n')) assert.ok(Buffer.byteLength(line)<=75);
});
test('invalid mappings and incomplete rows cannot create contacts', () => {
 assert.deepEqual(contactsFromRows([['Alex','123']],0,0),[]);
 assert.deepEqual(contactsFromRows([['Alex','123']],-1,1),[]);
 assert.deepEqual(contactsFromRows([['','123'],['Alex','']],0,1),[]);
});

test('optional name additions apply once, preserve phones, and exclude incomplete contacts', () => {
 const rows = [['Alex', '001234'], ['', '+123'], ['Sam', '']];
 assert.deepEqual(contactsFromRows(rows, 0, 1, ' Team ', ' Office '), [{name:'Team Alex Office',phone:'001234'}]);
 assert.equal(contactsFromRows(rows, 0, 1, '  ', '')[0].name, 'Alex');
 assert.equal(contactsFromRows(rows, 0, 1, 'Team')[0].name, 'Team Alex');
 assert.equal(contactsFromRows(rows, 0, 1, '', 'Office')[0].name, 'Alex Office');
 assert.equal(contactsFromRows(rows, 0, 1)[0].name, 'Alex');
 const vcf = toVcf(contactsFromRows(rows, 0, 1, 'Team;', 'Office,'));
 assert.ok(vcf.includes('FN:Team\\; Alex Office\\,\r\n'));
 assert.ok(vcf.includes('N:Team\\; Alex Office\\,;;;;\r\n'));
 assert.ok(vcf.includes('TEL;TYPE=CELL:001234\r\n'));
});
