<?php
namespace App\Http\Middleware;

use Closure;
use Illuminate\Http\Request;
use Illuminate\Support\Facades\Cache;
use Firebase\JWT\JWT;
use Firebase\JWT\Key;

class JwtAuth
{
    public function handle(Request $request, Closure $next)
    {
        $auth = $request->header('Authorization','');
        if (!str_starts_with($auth, 'Bearer ')) {
            return response()->json(['message'=>'Missing Bearer token'], 401);
        }
        $token = substr($auth, 7);

        try {
            
            [$h] = explode('.', $token);
            $header = JWT::jsonDecode(JWT::urlsafeB64Decode($h));
            $kid = $header->kid ?? null;
            if (!$kid) throw new \Exception('No kid in token');

            
            $jwks = Cache::remember('jwks_cache', 600, function () {
                $jwksUrl = config('services.auth.jwks_url') ?? env('AUTH_JWKS_URL');
                if (!$jwksUrl) throw new \Exception('AUTH_JWKS_URL not configured');
                $json = @file_get_contents($jwksUrl);
                if ($json === false) throw new \Exception('Cannot download JWKS');
                return json_decode($json, true);
            });

            $jwk = collect($jwks['keys'] ?? [])->firstWhere('kid', $kid);
            if (!$jwk) throw new \Exception('kid not found in JWKS');

            
            $publicKeyPem = $this->jwkToPem($jwk);
            $decoded = JWT::decode($token, new Key($publicKeyPem, 'RS256'));

            
            if ($iss = env('JWT_ISSUER')) {
                if (($decoded->iss ?? null) !== $iss) throw new \Exception('Bad iss');
            }
            if ($aud = env('JWT_AUDIENCE')) {
                $tokAud = $decoded->aud ?? null;
                $ok = is_array($tokAud) ? in_array($aud, $tokAud) : ($tokAud === $aud);
                if (!$ok) throw new \Exception('Bad aud');
            }

            
            $request->attributes->set('jwt_claims', (array) $decoded);

            return $next($request);
        } catch (\Throwable $e) {
            return response()->json(['message'=>'Invalid token','detail'=>$e->getMessage()], 401);
        }
    }

    private function jwkToPem(array $jwk): string
    {
        $n = $this->urlsafeB64ToBin($jwk['n']);
        $e = $this->urlsafeB64ToBin($jwk['e']);
        $seq = $this->asn1Sequence($this->asn1Integer($n) . $this->asn1Integer($e));
        $bitString = "\x03" . chr(strlen($seq)+1) . "\x00" . $seq;
        $algoId = hex2bin('300D06092A864886F70D0101010500'); // rsaEncryption OID
        $pubKeySeq = "\x30" . chr(strlen($algoId.$bitString)) . $algoId . $bitString;
        return "-----BEGIN PUBLIC KEY-----\n" .
               chunk_split(base64_encode($pubKeySeq), 64, "\n") .
               "-----END PUBLIC KEY-----\n";
    }

    private function urlsafeB64ToBin(string $val): string
    {
        $remainder = strlen($val) % 4;
        if ($remainder) $val .= str_repeat('=', 4 - $remainder);
        return base64_decode(strtr($val, '-_', '+/'));
    }

    private function asn1Integer(string $bin): string
    {
        if ($bin !== '' && ((ord($bin[0]) & 0x80) !== 0)) $bin = "\x00".$bin;
        return "\x02" . chr(strlen($bin)) . $bin;
    }
    private function asn1Sequence(string $bin): string
    {
        return "\x30" . chr(strlen($bin)) . $bin;
    }
}
