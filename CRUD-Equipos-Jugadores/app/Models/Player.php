<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;
use Illuminate\Database\Eloquent\Factories\HasFactory;

class Player extends Model
{
    use HasFactory;

    protected $fillable = [
        'team_id','full_name','number','position','height','age','nationality'
    ];

    protected $casts = [
        'number' => 'integer',
        'age'    => 'integer',
        'height' => 'float',
    ];

    public function team()
    {
        return $this->belongsTo(Team::class);
    }
}


