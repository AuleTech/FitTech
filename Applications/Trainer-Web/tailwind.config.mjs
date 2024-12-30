/** @type {import('tailwindcss').Config} */
import { palettes, rounded } from "@tailus/themer";
export default {
  content: [
    "./src/**/*.{astro,html,js,jsx,md,mdx,svelte,ts,tsx,vue}",
    "./node_modules/@tailus/themer/dist/components/**/*.{js,ts}",
  ],
  theme: {
    extend: {
      colors: palettes.trust,
    },
  },
  plugins: [rounded],
};
