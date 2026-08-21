import { readFile } from 'node:fs/promises';

const bandStyles = await readFile(
  'src/app/features/band-detail/band-hero/band-hero.component.scss',
  'utf8',
);
const releaseStyles = await readFile(
  'src/app/features/release-detail/release-hero/release-hero.component.scss',
  'utf8',
);

const requiredRules = [
  ['band photo frame adapts to the image ratio', /&__photo-wrap\s*\{[\s\S]*?width:\s*fit-content/],
  ['band photo frame is not stretched by the detail layout', /&__photo-wrap\s*\{[\s\S]*?align-self:\s*flex-start/],
  ['band photo has bounded responsive dimensions', /&__photo\s*\{[\s\S]*?max-width:\s*clamp\([\s\S]*?max-height:\s*clamp\(/],
  ['band photo preserves the whole uploaded image', /&__photo\s*\{[\s\S]*?object-fit:\s*contain/],
  ['band logo uses a bounded responsive height', /&-img\s*\{[\s\S]*?height:\s*clamp\(/],
  ['band logo preserves the whole uploaded image', /&-img\s*\{[\s\S]*?object-fit:\s*contain/],
  ['release cover uses a bounded responsive width', /&__cover-wrap\s*\{[\s\S]*?width:\s*clamp\(/],
  ['release cover preserves the whole uploaded image', /&__cover\s*\{[\s\S]*?object-fit:\s*contain/],
];

for (const [description, pattern] of requiredRules) {
  const source = description.startsWith('release') ? releaseStyles : bandStyles;
  if (!pattern.test(source)) {
    throw new Error(`Missing responsive media rule: ${description}.`);
  }
}
