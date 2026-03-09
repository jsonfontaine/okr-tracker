const genialPreset = require('@genial/design-system/tailwind-preset');

/** @type {import('tailwindcss').Config} */
module.exports = {
  presets: [genialPreset],
  corePlugins: {
    preflight: false,
  },
  content: [
    './src/**/*.{js,jsx,ts,tsx}',
    './node_modules/@genial/design-system/dist/**/*.{js,mjs}',
  ],
  theme: {
    extend: {},
  },
  plugins: [],
};
