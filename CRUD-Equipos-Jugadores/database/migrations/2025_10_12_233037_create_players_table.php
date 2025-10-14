<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

return new class extends Migration {
    public function up(): void
{
    Schema::create('players', function (Blueprint $table) {
        $table->id();
        $table->foreignId('team_id')->constrained('teams')->onDelete('cascade');
        $table->string('full_name', 200);
        $table->unsignedTinyInteger('number');     
        $table->string('position', 2);             
        $table->float('height');                   
        $table->unsignedTinyInteger('age');
        $table->string('nationality', 80);
        $table->timestamps();

        $table->unique(['team_id','number']);     
    });
}

    public function down(): void
    {
        Schema::dropIfExists('players');
    }
};
