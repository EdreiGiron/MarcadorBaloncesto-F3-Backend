<?php

namespace Database\Seeders;

use Illuminate\Database\Console\Seeds\WithoutModelEvents;
use Illuminate\Database\Seeder;

class PlayerSeeder extends Seeder
{
    /**
     * Run the database seeds.
     */
public function run(): void
{
    $positions = ['PG','SG','SF','PF','C'];

    \App\Models\Team::all()->each(function ($team) use ($positions) {
        $numbers = range(4, 15); // 12 jugadores
        foreach ($numbers as $n) {
            \App\Models\Player::firstOrCreate(
                ['team_id' => $team->id, 'number' => $n],
                [
                    'full_name'   => fake()->name(),
                    'position'    => $positions[array_rand($positions)],
                    'height'      => fake()->randomFloat(2, 1.75, 2.12),
                    'age'         => fake()->numberBetween(18, 38),
                    'nationality' => fake()->country(),
                ]
            );
        }
    });
}
}
