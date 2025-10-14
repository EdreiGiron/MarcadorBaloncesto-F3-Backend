<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;
use Illuminate\Database\Eloquent\Factories\HasFactory;

class Team extends Model
{
    use HasFactory;

    protected $fillable = ['name','city','logo_url'];

    public function players()
    {
        return $this->hasMany(Player::class);
    }
}
