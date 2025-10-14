<?php

namespace App\Http\Controllers;

use App\Models\Player;
use Illuminate\Http\Request;
use App\Http\Requests\PlayerStoreRequest;
use App\Http\Requests\PlayerUpdateRequest;

class PlayerController extends Controller
{
    public function index(Request $r)
    {
        $q = Player::query()->with('team:id,name');

        if ($r->filled('teamId')) {
            $q->where('team_id', $r->integer('teamId'));
        }
        if ($r->filled('search')) {
            $s = $r->get('search');
            $q->where('full_name','like',"%$s%");
        }

        return $q->orderBy('team_id')->orderBy('number')
                 ->paginate($r->integer('per_page', 10));
    }

    public function store(PlayerStoreRequest $req)
    {
        $player = Player::create($req->validated());
        return response()->json($player, 201);
    }

    public function show(Player $player)
    {
        return $player->load('team:id,name');
    }

    public function update(PlayerUpdateRequest $req, Player $player)
    {
        $player->update($req->validated());
        return $player->load('team:id,name');
    }

    public function destroy(Player $player)
    {
        $player->delete();
        return response()->noContent();
    }
}
