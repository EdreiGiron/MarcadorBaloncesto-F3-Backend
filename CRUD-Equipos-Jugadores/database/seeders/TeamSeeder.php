<?php

namespace Database\Seeders;

use Illuminate\Database\Console\Seeds\WithoutModelEvents;
use Illuminate\Database\Seeder;

class TeamSeeder extends Seeder
{
    /**
     * Run the database seeds.
     */
public function run(): void
{
    $teams = [
        ['name' => 'Tigres',    'city' => 'Guatemala',       'logo_url' => null],
        ['name' => 'Leones',    'city' => 'Quetzaltenango',  'logo_url' => null],
        ['name' => 'Halcones',  'city' => 'Escuintla',       'logo_url' => null],
        ['name' => 'Toros',     'city' => 'Antigua',         'logo_url' => null],
        ['name' => 'Guerreros', 'city' => 'Cobán',           'logo_url' => null],
    ];

    foreach ($teams as $t) {
        \App\Models\Team::firstOrCreate(
            ['name' => $t['name'], 'city' => $t['city']],
            ['logo_url' => $t['logo_url']]
        );
    }
}
}
