<?php

namespace Database\Factories;

use Illuminate\Database\Eloquent\Factories\Factory;

/**
 * @extends \Illuminate\Database\Eloquent\Factories\Factory<\App\Models\Player>
 */
class PlayerFactory extends Factory
{
    /**
     * Define the model's default state.
     *
     * @return array<string, mixed>
     */
public function definition(): array
{
    $positions = ['PG','SG','SF','PF','C'];

    return [
        'team_id'     => \App\Models\Team::factory(), // se sobreescribe en el seeder
        'full_name'   => fake()->name(),
        'number'      => fake()->numberBetween(0, 99),
        'position'    => fake()->randomElement($positions),
        'height'      => fake()->randomFloat(2, 1.70, 2.15),
        'age'         => fake()->numberBetween(18, 38),
        'nationality' => fake()->country(),
    ];
}
}
