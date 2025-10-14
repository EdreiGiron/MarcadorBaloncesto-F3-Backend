<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;
use Illuminate\Database\Eloquent\Relations\BelongsTo;

class Player extends Model
{
    protected $fillable = [
        'team_id','number','full_name','position','nationality','height_cm','weight_kg'
    ];

    protected $casts = [
        'height_cm' => 'integer',
        'weight_kg' => 'integer',
    ];

    public function team(): BelongsTo
    {
        return $this->belongsTo(Team::class);
    }
}

