<?php

namespace App\Http\Controllers;

use App\Models\Player;
use Illuminate\Http\Request;

class PlayerController extends Controller
{
    public function index()
    {
        return response()->json( Player::orderBy('id')->get() );
    }

    public function show(Player $player)
    {
        return response()->json( $player->load('team') );
    }

    public function store(Request $request)
    {
        $data = $request->validate([
            'team_id'     => 'required|integer|exists:teams,id',
            'full_name'   => 'required|string|max:200',
            'number'      => 'required|integer|min:0',
            'position'    => 'required|string|max:10',
            'height'      => 'required|numeric',
            'age'         => 'required|integer|min:0',
            'nationality' => 'required|string|max:100',
        ]);
        $player = Player::create($data);
        return response()->json($player, 201);
    }

    public function update(Request $request, Player $player)
    {
        $data = $request->validate([
            'team_id'     => 'sometimes|integer|exists:teams,id',
            'full_name'   => 'sometimes|string|max:200',
            'number'      => 'sometimes|integer|min:0',
            'position'    => 'sometimes|string|max:10',
            'height'      => 'sometimes|numeric',
            'age'         => 'sometimes|integer|min:0',
            'nationality' => 'sometimes|string|max:100',
        ]);
        $player->fill($data)->save();
        return response()->json($player);
    }

    public function destroy(Player $player)
    {
        $player->delete();
        return response()->json(['deleted' => true]);
    }
}
