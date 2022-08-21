// SPDX-FileCopyrightText: 2022 Simon Wendel
// SPDX-License-Identifier: GPL-3.0-or-later

const CONFIGURATION = {
    elmProgram: 'src/Main.elm',
    elmSources: 'src/**/*.elm',
    elmTests: 'tests/**/*.elm',
    elmBundleFile: 'app.js',
    logo: 'img/logos/logo.png',
    staticAssets: ['index.html', 'img/*', 'js/**/*.js'],
    lessFiles: ['style/**/*.less'],
    outputDirectory: 'dist/'
}

const { src, dest, series, watch } = require('gulp');
const elm = require('gulp-elm');
const less = require('gulp-less');
const concat = require('gulp-concat');
const del = require('del');
const shell = require('gulp-shell');
const rename = require('gulp-rename');

function cleanDistFolder() {
    return del(CONFIGURATION.outputDirectory + '**');
}

function copyLogo() {
    return src(CONFIGURATION.logo)
        .pipe(rename('logo.png'))
        .pipe(dest(CONFIGURATION.outputDirectory))
}

function copyStaticAssets() {
    return src(CONFIGURATION.staticAssets)
        .pipe(dest(CONFIGURATION.outputDirectory));
}

function makeCssFiles() {
    return src(CONFIGURATION.lessFiles)
        .pipe(less())
        .pipe(concat('app.css'))
        .pipe(dest(CONFIGURATION.outputDirectory));
}

function makeElmBundle() {
    return src(CONFIGURATION.elmProgram)
        .pipe(elm.bundle(CONFIGURATION.elmBundleFile))
        .pipe(dest(CONFIGURATION.outputDirectory));
}

function runTests() {
    return shell.task('elm-test')();
}

function watchTests() {
    return shell.task('elm-test --watch')();
}

const buildTask = series(copyLogo, copyStaticAssets, makeCssFiles, makeElmBundle);
exports.build = buildTask;

const testTask = runTests;
exports.test = testTask;

const defaultTask = series(runTests, cleanDistFolder, buildTask)
exports.default = defaultTask;

const watchTask = () => {
    const files = [CONFIGURATION.elmSources].concat(CONFIGURATION.staticAssets).concat(CONFIGURATION.lessFiles);
    const settings = { ignoreInitial: false };
    series(
        cleanDistFolder);
    watch(files, settings, buildTask);
    watchTests();
};
exports.watch = watchTask
