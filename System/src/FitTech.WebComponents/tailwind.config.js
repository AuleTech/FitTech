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
                'fittech-gray': '#DBD9DB',
                'fittech-dark': '#494947',
                'fittech-white': '#E5EBEA',
                'fittech-gray-secondary': '#979797',
                'fittech-red': '#EF5D60',
                'fittech-yellow': '#EFEF5D',
            }
        }
    },
    plugins: []
};
