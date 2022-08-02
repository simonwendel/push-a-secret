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

function configureElmApp() {
    const env = process.env.ELM_ENVIRONMENT ?? 'default';
    return src('env/Configuration.elm_' + env)
        .pipe(rename('Configuration.elm'))
        .pipe(dest('src', { overwrite: true }));
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

const buildTask = series(cleanDistFolder, copyLogo, copyStaticAssets, makeCssFiles, configureElmApp, makeElmBundle);
exports.build = buildTask;

const testTask = runTests;
exports.test = testTask;

const watchTask = () => {
    const files = [CONFIGURATION.elmSources].concat(CONFIGURATION.staticAssets).concat(CONFIGURATION.lessFiles);
    const settings = { ignoreInitial: false };
    watch(files, settings, buildTask);
    watchTests();
};

exports.watch = watchTask
exports.default = series(runTests, buildTask);
