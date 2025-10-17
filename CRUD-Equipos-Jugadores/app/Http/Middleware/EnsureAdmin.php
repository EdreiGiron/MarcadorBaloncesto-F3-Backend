<?php

namespace App\Http\Middleware;

use Closure;
use Illuminate\Http\Request;

class EnsureAdmin
{
    public function handle(Request $request, Closure $next)
    {
        $claims = $request->attributes->get('jwt_claims', []);

        $role = null;

    
        if (isset($claims['role'])) {
            $role = $claims['role'];
        }

 
        if (isset($claims['roles']) && is_string($claims['roles'])) {
            $role = explode(',', $claims['roles'])[0]; 
        }

       
        if (isset($claims['roles']) && is_array($claims['roles'])) {
            $role = $claims['roles'][0];
        }

      
        if (isset($claims['realm_access']['roles'][0])) {
            $role = $claims['realm_access']['roles'][0];
        }

   
        $normalizedRole = strtolower(trim($role));

      
        if ($normalizedRole !== 'admin') {
            return response()->json([
                'message' => 'Forbidden: Only Admin can modify resources',
                'detected_role' => $role,
                'raw_roles_claim' => $claims['roles'] ?? '(no roles found)'
            ], 403);
        }

        return $next($request);
    }
}
