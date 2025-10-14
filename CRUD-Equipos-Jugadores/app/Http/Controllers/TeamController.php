<?php

namespace App\Http\Controllers;

use App\Models\Team;
use Illuminate\Http\Request;
use App\Http\Requests\TeamStoreRequest;
use App\Http\Requests\TeamUpdateRequest;

class TeamController extends Controller
{
    public function index(Request $r)
    {
        $q = Team::query()
            ->withCount('players')
            ->when($r->get('search'), fn($qq,$s) => $qq->where('name','like',"%$s%"));

        return $q->orderBy('name')->paginate($r->integer('per_page', 10));
    }

    public function store(TeamStoreRequest $req)
    {
        $team = Team::create($req->validated());
        return response()->json($team, 201);
    }

    public function show(Team $team)
    {
        return $team->load('players');
    }

    public function update(TeamUpdateRequest $req, Team $team)
    {
        $team->update($req->validated());
        return $team;
    }

    public function destroy(Team $team)
    {
        $team->delete();
        return response()->noContent();
    }
}