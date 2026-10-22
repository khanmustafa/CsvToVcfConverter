'use strict';
const $ = id => document.getElementById(id);
let workbook, rows = [], contacts = [], filename = '', requestId = 0;
function status(message, error = false) {
  $('status').textContent = message;
  $('status').classList.toggle('error', error);
}
function updatePreview() {
  const name = Number($('name-column').value), phone = Number($('phone-column').value);
  contacts = ContactConverter.contactsFromRows(rows, name, phone);
  $('preview').replaceChildren();
  for (const contact of contacts.slice(0, 5)) {
    const row = document.createElement('tr');
    for (const value of [contact.name, contact.phone]) {
      const cell = document.createElement('td'); cell.textContent = value; row.append(cell);
    }
    $('preview').append(row);
  }
  $('count').textContent = `${contacts.length.toLocaleString()} contacts`;
  $('download').disabled = contacts.length === 0;
  $('skipped').textContent = name === phone && name >= 0 ? 'Choose different columns for name and phone.' :
    name < 0 || phone < 0 ? 'Choose both columns to see your contacts.' :
    `Showing up to 5 contacts. ${rows.length - contacts.length} rows without both a name and phone will be skipped.`;
}
function selectSheet() {
  const data = XLSX.utils.sheet_to_json(workbook.Sheets[$('sheet').value], { header: 1, raw: false, defval: '', blankrows: false });
  const headers = data.shift() || [];
  rows = data;
  if (!headers.length || !rows.length) throw new Error('This sheet needs a header row and at least one contact.');
  for (const id of ['name-column', 'phone-column']) {
    const select = $(id); select.replaceChildren(new Option('Choose a column', '-1'));
    Array.from(headers).forEach((header, i) => select.add(new Option(`${String(header).trim() || 'Unnamed column'} (${i + 1})`, String(i))));
  }
  const nameIndex = headers.findIndex(h => /^(formatted name|name|full name|contact name)$/i.test(String(h).trim()));
  const phoneIndex = headers.findIndex(h => /phone|mobile|cell|whatsapp|contact number/i.test(String(h)));
  $('name-column').value = String(nameIndex); $('phone-column').value = String(phoneIndex);
  $('mapping').hidden = false; updatePreview();
}
async function loadFile(file) {
  const id = ++requestId;
  $('mapping').hidden = true; contacts = []; workbook = null;
  if (!file) return;
  try {
    if (!/\.(csv|xlsx|xls)$/i.test(file.name)) throw new Error('Choose a CSV, XLSX, or XLS file.');
    if (file.size > 20 * 1024 * 1024) throw new Error('Please choose a file smaller than 20 MB.');
    if (!file.size) throw new Error('This file is empty. Choose a file with contact data.');
    status(`Reading ${file.name}…`);
    const buffer = await file.arrayBuffer();
    if (id !== requestId) return;
    const csv = /\.csv$/i.test(file.name);
    workbook = XLSX.read(csv ? new TextDecoder().decode(buffer) : buffer, { type: csv ? 'string' : 'array', raw: csv, cellText: true, cellDates: false });
    filename = file.name;
    $('sheet').replaceChildren(...workbook.SheetNames.map(name => new Option(name, name)));
    $('sheet-field').hidden = workbook.SheetNames.length < 2;
    selectSheet();
    status(`${file.name} · Read locally. Review your columns below.`);
  } catch (error) {
    if (id !== requestId) return;
    $('mapping').hidden = true;
    status(error.message && /Choose|choose|sheet|file|header/.test(error.message) ? error.message : 'Could not read this file. Check that it is a valid, unprotected spreadsheet.', true);
  }
}
$('file').addEventListener('change', event => { loadFile(event.target.files[0]); event.target.value = ''; });
for (const id of ['name-column', 'phone-column']) $(id).addEventListener('change', updatePreview);
$('sheet').addEventListener('change', () => {
  try { selectSheet(); status(`${filename} · Read locally. Review your columns below.`); }
  catch (error) { $('mapping').hidden = false; rows = []; updatePreview(); status(error.message, true); }
});
for (const type of ['dragover', 'dragleave', 'drop']) $('dropzone').addEventListener(type, event => {
  event.preventDefault(); $('dropzone').classList.toggle('dragging', type === 'dragover');
  if (type === 'drop') loadFile(event.dataTransfer.files[0]);
});
$('download').addEventListener('click', () => {
  if (!contacts.length) return;
  const url = URL.createObjectURL(new Blob([ContactConverter.toVcf(contacts)], { type: 'text/vcard;charset=utf-8' }));
  const link = document.createElement('a'); link.href = url; link.download = filename.replace(/\.[^.]+$/, '') + '.vcf';
  document.body.append(link); link.click(); link.remove(); setTimeout(() => URL.revokeObjectURL(url), 1000);
  status(`Your VCF is ready: ${contacts.length.toLocaleString()} contacts. You can download it again or choose another file.`);
});
