/** @type {import('tailwindcss').Config} */
module.exports = {
    darkMode: 'class',
    content: [
        './../**/*.{razor,html,cs,razor.css,cshtml}'
    ],
    theme: {
        extend: {
            colors: {
                'fittech-green': 'rgba(173,197,57,1)',
                'fittech-gray': 'rgba(73,73,71,1)',
                'fittech-dark': 'rgba(0, 0, 0, 1)',
                'fittech-white': 'rgba(229, 235, 234, 1)',
                'fittech-grayvibrant': 'rgba(140, 140, 140, 1)',
            }
        }
    },
    plugins: []
};
