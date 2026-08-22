import { readFile } from 'node:fs/promises';

const componentPaths = [
  {
    styles: 'src/app/features/band-detail/back-to-results/back-to-results.component.scss',
    template: 'src/app/features/band-detail/back-to-results/back-to-results.component.html',
  },
  {
    styles: 'src/app/features/release-detail/back-to-band/back-to-band.component.scss',
    template: 'src/app/features/release-detail/back-to-band/back-to-band.component.html',
  },
];

for (const { styles: stylePath, template: templatePath } of componentPaths) {
  const [styles, template] = await Promise.all([
    readFile(stylePath, 'utf8'),
    readFile(templatePath, 'utf8'),
  ]);
  const iconBoxRule = /&__arrow\s*\{[\s\S]*?display:\s*inline-flex[\s\S]*?width:\s*1rem[\s\S]*?height:\s*1rem/;
  const svgRule = /<svg\s+class="back-nav__arrow-icon"\s+viewBox="0 0 24 24"/;

  if (!iconBoxRule.test(styles) || !svgRule.test(template) || /translateY/.test(styles)) {
    throw new Error(`Back-navigation arrow is not scale-independent: ${stylePath}.`);
  }
}
