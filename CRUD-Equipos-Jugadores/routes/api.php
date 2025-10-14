<?php

use Illuminate\Support\Facades\Route;
use App\Http\Controllers\TeamController;
use App\Http\Controllers\PlayerController;

Route::get('/health', fn() => response()->json(['status'=>'ok']));

// TEAMS
Route::get('/teams', [TeamController::class, 'index']);
Route::get('/teams/{team}', [TeamController::class, 'show']);
Route::post('/teams', [TeamController::class, 'store']);
Route::match(['put','patch'],'/teams/{team}', [TeamController::class, 'update']);
Route::delete('/teams/{team}', [TeamController::class, 'destroy']);

// PLAYERS
Route::get('/players', [PlayerController::class, 'index']);
Route::get('/players/{player}', [PlayerController::class, 'show']);
Route::post('/players', [PlayerController::class, 'store']);
Route::match(['put','patch'],'/players/{player}', [PlayerController::class, 'update']);
Route::delete('/players/{player}', [PlayerController::class, 'destroy']);

Route::get('/teams/{team}/players', function (\App\Models\Team $team) {
    return response()->json($team->players()->orderBy('id')->get());
});
