<?php

namespace App\Http\Requests;

use Illuminate\Foundation\Http\FormRequest;

class TeamStoreRequest extends FormRequest
{
    public function authorize(): bool { return true; }

    public function rules(): array
    {
        return [
            'name'     => ['required','string','max:120','unique:teams,name'],
            'city'     => ['nullable','string','max:120'],
            'logo_url' => ['nullable','url','max:255'],
        ];
    }
}
