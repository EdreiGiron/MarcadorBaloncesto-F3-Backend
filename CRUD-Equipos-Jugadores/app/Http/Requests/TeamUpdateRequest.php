<?php

namespace App\Http\Requests;

use Illuminate\Foundation\Http\FormRequest;
use Illuminate\Validation\Rule;

class TeamUpdateRequest extends FormRequest
{
    public function authorize(): bool { return true; }

    public function rules(): array
    {

        $id = is_numeric($this->route('team')) ? $this->route('team') : $this->route('team')->id;

        return [
            'name'     => ['sometimes','string','max:120', Rule::unique('teams','name')->ignore($id)],
            'city'     => ['sometimes','nullable','string','max:120'],
            'logo_url' => ['sometimes','nullable','url','max:255'],
        ];
    }
}
