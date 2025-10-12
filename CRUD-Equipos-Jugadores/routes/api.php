<?php

use Illuminate\Support\Facades\Route;

Route::get('/health', fn() => response()->json(['status'=>'ok']));

Route::middleware(['auth.jwt'])->group(function () {
    Route::get('/ping-protegido', fn() => response()->json(['pong'=>true]));
});
