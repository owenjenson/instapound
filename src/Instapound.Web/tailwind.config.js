/** @type {import('tailwindcss').Config} */

const colors = require('tailwindcss/colors')

module.exports = {
    content: ["./Views/**/*.{cshtml,razor}", "./wwwroot/js/**/*.js"],
    theme: {
        colors: {
            current: colors.current,
            transparent: colors.transparent,
            white: colors.white,
            black: colors.black,
            gray: colors.gray,
            inherit: colors.inherit,

            primary: 'var(--primary)',
            'primary-dim': 'var(--primary-dim)',
            'on-primary': 'var(--on-primary)',
            'on-primary-dim': 'var(--on-primary-dim)',
            secondary: 'var(--secondary)',
            'on-secondary': 'var(--on-secondary)',
            'secondary-dim': 'var(--secondary-dim)',
            'on-secondary-dim': 'var(--on-secondary-dim)',

            surface: 'var(--surface)',
            'on-surface': 'var(--on-surface)',
            'on-surface-muted': 'var(--on-surface-muted)',

            'surface-dim': 'var(--surface-dim)',
            'on-surface-dim': 'var(--on-surface-dim)',
            'on-surface-dim-muted': 'var(--on-surface-dim-muted)',

            'surface-container': 'var(--surface-container)',
            'on-surface-container': 'var(--on-surface-container)',
            'on-surface-container-muted': 'var(--on-surface-container-muted)',

            'surface-dim-container': 'var(--surface-dim-container)',
            'on-surface-dim-container': 'var(--on-surface-dim-container)',
            'on-surface-dim-container-muted': 'var(--on-surface-dim-container-muted)',

            outline: 'var(--outline)',
            'outline-variant': 'var(--outline-variant)',

            success: 'var(--success)',
            'on-success': 'var(--on-success)',

            danger: 'var(--danger)',
            'on-danger': 'var(--on-danger)',
            'danger-dim': 'var(--danger-dim)',
            'on-danger-dim': 'var(--on-danger-dim)',

            shade: 'var(--shade)',

            backdrop: 'var(--backdrop)',
        },
        extend: {},
    },
    plugins: [],
}