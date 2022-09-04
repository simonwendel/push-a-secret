// SPDX-FileCopyrightText: 2022 Simon Wendel
// SPDX-License-Identifier: GPL-3.0-or-later

const buildingForProduction = process.env.ELM_ENVIRONMENT === 'Production';

const CONFIGURATION = {
    sources: 'src/**/*.elm',
    tests: 'tests/**/*.elm',

    entrypoint: 'index.js',
    bundle: 'app.js',

    logo: 'img/logos/logo.png',
    smallLogo: 'img/logos/logo_small.png',

    html: 'index.html',
    less: 'style/**/*.less',
    js: 'js/**/*.js',
    images: 'img/*',

    output: 'dist/'
}

const {
    src,
    dest,
    series,
    watch,
    parallel
} = require('gulp');

const less = require('gulp-less');
const concat = require('gulp-concat');
const del = require('del');
const shell = require('gulp-shell');
const rename = require('gulp-rename');
const CleanCSS = require('clean-css');
const minifier = require('html-minifier');
const esbuild = require('gulp-esbuild')
const ElmPlugin = require('esbuild-plugin-elm');

const clean = () =>
    del(CONFIGURATION.output + '**');

const logo = () =>
    src(CONFIGURATION.logo)
        .pipe(rename('logo.png'))
        .pipe(dest(CONFIGURATION.output));

const responsiveLogo = () =>
    src(CONFIGURATION.smallLogo)
        .pipe(rename('logo_small.png'))
        .pipe(dest(CONFIGURATION.output));

const favicon = () =>
    src('img/favicon.ico')
        .pipe(dest(CONFIGURATION.output));

const scripts = () =>
    src(CONFIGURATION.entrypoint)
        .pipe(esbuild({
            outfile: CONFIGURATION.bundle,
            bundle: true,
            minify: buildingForProduction,
            treeShaking: buildingForProduction,
            pure: [
                'A2',
                'A3',
                'A4',
                'A5',
                'A6',
                'A7',
                'A8',
                'A9',
                'F2',
                'F3',
                'F3',
                'F4',
                'F5',
                'F6',
                'F7',
                'F8',
                'F9'
            ],
            plugins: [
                ElmPlugin({
                    debug: !buildingForProduction,
                    optimize: buildingForProduction
                })
            ]
        }))
        .pipe(dest(CONFIGURATION.output));

const html = () =>
    src(CONFIGURATION.html)
        .on('data', function (file) {
            const options = {
                html5: true,
                includeAutoGeneratedTags: true,
                removeAttributeQuotes: true,
                removeComments: false,
                removeRedundantAttributes: true,
                removeScriptTypeAttributes: true,
                removeStyleLinkTypeAttributes: true,
                sortClassName: true,
                useShortDoctype: true,
                collapseWhitespace: true,
            };

            const buferFile = Buffer.from(minifier.minify(file.contents.toString(), options))
            return file.contents = buferFile
        })
        .pipe(dest(CONFIGURATION.output))

const css = () =>
    src(CONFIGURATION.less)
        .pipe(less())
        .pipe(concat('app.css'))
        .on('data', function (file) {
            const options = {
                compatibility: '*',
                inline: ['all'],
                level: 2
            };

            const buffer = new CleanCSS(options).minify(file.contents)
            return file.contents = Buffer.from(buffer.styles)
        })
        .pipe(dest(CONFIGURATION.output));

const test = () =>
    shell.task('elm-test')();

exports.clean = clean;
exports.css = css;
exports.html = html;
exports.scripts = scripts;
exports.test = test;

exports.gfx = parallel(logo, responsiveLogo, favicon);
exports.all = parallel(exports.gfx, html, css, scripts);

exports.default = series(test, clean, exports.all);
exports.watch = () => {
    const files = [
        CONFIGURATION.sources,
        CONFIGURATION.tests,
        CONFIGURATION.html,
        CONFIGURATION.less,
        CONFIGURATION.js];
    const settings = { ignoreInitial: false };
    clean();
    watch(files, settings, exports.all);
    shell.task('elm-test --watch')();
};
