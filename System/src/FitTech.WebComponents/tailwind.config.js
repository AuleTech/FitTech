/** @type {import('tailwindcss').Config} */
module.exports = {
    darkMode: 'class',
    content: [
        './../**/*.{razor,html,cs,razor.css,cshtml}'
    ],
    theme: {
        extend: {
            colors: {
                'fittech-primary': 'rgba(173,197,57,1)',
                'fittech-secondary': 'rgba(73,73,71,1)',
                'fittech-tertiary': 'rgba(229, 235, 234, 1)'
            }
        }
    },
    plugins: []
};
