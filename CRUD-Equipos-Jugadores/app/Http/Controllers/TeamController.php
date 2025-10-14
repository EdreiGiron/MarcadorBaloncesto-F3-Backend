<?php

namespace App\Http\Controllers;

use App\Models\Team;
use Illuminate\Http\Request;

class TeamController extends Controller
{
    public function index()
    {
        // si quieres incluir jugadores: Team::with('players')->get();
        return response()->json( Team::orderBy('id')->get() );
    }

    public function show(Team $team)
    {
        return response()->json( $team->load('players') );
    }

    public function store(Request $request)
    {
        $data = $request->validate([
            'name'     => 'required|string|max:150',
            'city'     => 'required|string|max:150',
            'logo_url' => 'nullable|url',
        ]);
        $team = Team::create($data);
        return response()->json($team, 201);
    }

    public function update(Request $request, Team $team)
    {
        $data = $request->validate([
            'name'     => 'sometimes|string|max:150',
            'city'     => 'sometimes|string|max:150',
            'logo_url' => 'nullable|url',
        ]);
        $team->fill($data)->save();
        return response()->json($team);
    }

    public function destroy(Team $team)
    {
        $team->delete();
        return response()->json(['deleted' => true]);
    }
}
