/** @type {import('tailwindcss').Config} */
module.exports = {
    darkMode: 'class',
    content: [
        './../**/*.{razor,html,cs,razor.css,cshtml}'
    ],
    theme: {
        extend: {
            colors: {
                'fittech-green': '#ADC539',
                'fittech-gray': '#494947',
                'fittech-dark': '#000000',
                'fittech-white': '#E5EBEA',
                'fittech-grayvibrant': '#8C8C8C',
                'fittech-red': '#EF5D60',
            }
        }
    },
    plugins: []
};
