<?php

namespace App\Http\Requests;

use Illuminate\Foundation\Http\FormRequest;
use Illuminate\Validation\Rule;

class PlayerUpdateRequest extends FormRequest
{
    public function authorize(): bool { return true; }

    public function rules(): array
    {
        $player = $this->route('player');
        $playerId = is_numeric($player) ? $player : $player->id;

        $teamId = $this->input('team_id', is_numeric($player) ? null : $player->team_id);

        return [
            'team_id'    => ['sometimes','exists:teams,id'],
            'number'     => [
                'sometimes','integer','between:0,99',
                Rule::unique('players','number')
                    ->ignore($playerId)
                    ->where(fn($q) => $q->where('team_id', $teamId ?? $this->team_id)),
            ],
            'full_name'  => ['sometimes','string','max:120'],
            'position'   => ['sometimes', Rule::in(['Base','Escolta','Alero','Ala-Pívot','Pívot'])],
            'nationality'=> ['sometimes','nullable','string','max:80'],
            'height_cm'  => ['sometimes','nullable','integer','between:120,250'],
            'weight_kg'  => ['sometimes','nullable','integer','between:45,160'],
        ];
    }
}
