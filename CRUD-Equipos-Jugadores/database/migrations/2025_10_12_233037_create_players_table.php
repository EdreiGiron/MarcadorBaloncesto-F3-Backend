<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

return new class extends Migration {
    public function up(): void
    {
        Schema::create('players', function (Blueprint $table) {
            $table->id();
            $table->foreignId('team_id')->constrained('teams')->cascadeOnDelete();
            $table->unsignedSmallInteger('number'); // dorsal 0..99, único por equipo
            $table->string('full_name');
            $table->enum('position', ['Base','Escolta','Alero','Ala-Pívot','Pívot']);
            $table->string('nationality')->nullable();
            $table->unsignedSmallInteger('height_cm')->nullable();
            $table->unsignedSmallInteger('weight_kg')->nullable();
            $table->timestamps();

            $table->unique(['team_id','number']);
        });
    }

    public function down(): void
    {
        Schema::dropIfExists('players');
    }
};
