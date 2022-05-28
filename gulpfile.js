const CONFIGURATION = {
    elmProgram: 'src/Main.elm',
    elmSources: 'src/**/*.elm',
    elmBundleFile: 'app.js',
    staticAssets: ['index.html', 'img/**/*', 'js/**/*.js'],
    lessFiles: ['style/**/*.less'],
    outputDirectory: 'dist/'
}

const { src, dest, series, watch } = require('gulp');
const elm = require('gulp-elm');
const less = require('gulp-less');
const concat = require('gulp-concat');
const del = require('del');

function cleanDistFolder() {
    return del(CONFIGURATION.outputDirectory + '**');
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

const buildChain = series(cleanDistFolder, copyStaticAssets, makeCssFiles, makeElmBundle);
exports.default = buildChain;
exports.watch = function () {
    watch(
        [CONFIGURATION.elmSources].concat(CONFIGURATION.staticAssets).concat(CONFIGURATION.lessFiles),
        { ignoreInitial: false },
        buildChain);
}
