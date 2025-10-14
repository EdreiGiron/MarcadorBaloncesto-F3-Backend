<?php
namespace App\Http\Middleware;

use Closure;
use Illuminate\Http\Request;

class EnsureAdmin
{
    public function handle(Request $request, Closure $next)
    {
        $claims = $request->attributes->get('jwt_claims', []);
        
        $role = $claims['role'] ?? ($claims['realm_access']['roles'][0] ?? null);
        if ($role !== 'Admin') {
            return response()->json(['message'=>'Forbidden: Admin required'], 403);
        }
        return $next($request);
    }
}
