const CONFIGURATION = {
    outputDirectory: 'dist/',
    elmFiles: 'src/**/*.elm',
    staticAssets: 'index.html',
    elmBundleFile: 'app.js'
}

const { src, dest, series, watch } = require('gulp');
const elm = require('gulp-elm');
const del = require('del');

function cleanDistFolder() {
    return del(CONFIGURATION.outputDirectory + '**');
}

function copyStaticAssets() {
    return src(CONFIGURATION.staticAssets)
        .pipe(dest(CONFIGURATION.outputDirectory));
}

function makeElmBundle() {
    return src(CONFIGURATION.elmFiles)
        .pipe(elm.bundle(CONFIGURATION.elmBundleFile))
        .pipe(dest(CONFIGURATION.outputDirectory));
}

const buildChain = series(cleanDistFolder, copyStaticAssets, makeElmBundle);
exports.default = buildChain;
exports.watch = function () {
    watch(CONFIGURATION.elmFiles, buildChain);
    watch(CONFIGURATION.staticAssets, buildChain);
}