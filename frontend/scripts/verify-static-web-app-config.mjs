import { access, readFile } from 'node:fs/promises';
import { constants } from 'node:fs';
import { join } from 'node:path';

const outputDirectory = join('dist', 'diy-music-community-web', 'browser');
const configurationPath = join(outputDirectory, 'staticwebapp.config.json');

await access(configurationPath, constants.R_OK);

const configuration = JSON.parse(await readFile(configurationPath, 'utf8'));

if (configuration.navigationFallback?.rewrite !== '/index.html') {
  throw new Error('Static Web Apps navigation fallback must rewrite to /index.html.');
}
