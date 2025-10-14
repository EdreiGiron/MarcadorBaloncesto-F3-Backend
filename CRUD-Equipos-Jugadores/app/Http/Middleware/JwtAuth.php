<?php

namespace App\Http\Middleware;

use Closure;
use Firebase\JWT\JWT;
use Firebase\JWT\Key;
use Illuminate\Support\Facades\Log;

class JwtAuth
{
    public function handle($request, Closure $next)
    {
        try {

            $auth = $request->headers->get('Authorization', '');
            $auth = trim((string) $auth);

            if ($auth === '') {
                return response()->json(['message' => 'Missing Bearer token'], 401);
            }

            $parts = preg_split('/\s+/', $auth);
            $type  = $parts[0] ?? '';
            $raw   = $parts[1] ?? '';

            if (strcasecmp($type, 'Bearer') !== 0 || $raw === '') {

                $raw = preg_replace('/^Bearer\s+/i', '', $auth);
            }

            $token = trim($raw, " \t\n\r\0\x0B\"'");

            Log::debug('JWT_HEADER_DEBUG', [
                'auth'     => $auth,
                'type'     => $type,
                'tokenLen' => strlen($token),
                'dotCount' => substr_count($token, '.'),
            ]);

            if (substr_count($token, '.') !== 2) {
                return response()->json(['message' => 'Invalid token', 'detail' => 'Bad JWT format'], 401);
            }

            $secret = env('JWT_HS256_SECRET');
            if (!$secret) {
                return response()->json(['message' => 'Invalid token', 'detail' => 'Missing HS256 secret'], 401);
            }

            $decoded = JWT::decode($token, new Key($secret, 'HS256'));

            $iss = env('JWT_ISSUER');
            $aud = env('JWT_AUDIENCE');

            if ($iss && (($decoded->iss ?? null) !== $iss)) {
                return response()->json(['message' => 'Invalid token', 'detail' => 'Bad iss'], 401);
            }

            if ($aud) {
                $tokAud = $decoded->aud ?? null;
                $ok = is_array($tokAud) ? in_array($aud, $tokAud) : ($tokAud === $aud);
                if (!$ok) {
                    return response()->json(['message' => 'Invalid token', 'detail' => 'Bad aud'], 401);
                }
            }

            $request->attributes->set('jwt_claims', (array) $decoded);

            return $next($request);

        } catch (\Throwable $e) {
            return response()->json([
                'message' => 'Invalid token',
                'detail'  => $e->getMessage(),
            ], 401);
        }
    }
}
