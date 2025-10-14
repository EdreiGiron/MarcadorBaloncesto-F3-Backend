<?php

namespace App\Http\Requests;

use Illuminate\Foundation\Http\FormRequest;
use Illuminate\Validation\Rule;

class PlayerStoreRequest extends FormRequest
{
    public function authorize(): bool { return true; }

    public function rules(): array
    {
        return [
            'team_id'    => ['required','exists:teams,id'],
            'number'     => [
                'required','integer','between:0,99',
                Rule::unique('players','number')->where(fn($q) => $q->where('team_id', $this->team_id)),
            ],
            'full_name'  => ['required','string','max:120'],
            'position'   => ['required', Rule::in(['Base','Escolta','Alero','Ala-Pívot','Pívot'])],
            'nationality'=> ['nullable','string','max:80'],
            'height_cm'  => ['nullable','integer','between:120,250'],
            'weight_kg'  => ['nullable','integer','between:45,160'],
        ];
    }
}
