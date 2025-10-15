package com.reports.client;

import com.reports.dto.*;
import lombok.RequiredArgsConstructor;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.http.ResponseEntity;
import org.springframework.stereotype.Component;
import org.springframework.web.client.RestTemplate;
import java.util.*;

@Component
@RequiredArgsConstructor
public class ExternalClient {

    private final RestTemplate restTemplate;

    @Value("${TEAMS_API_URL}")
    private String teamsUrl;

    @Value("${MATCHES_API_URL}")
    private String matchesUrl;

    public List<TeamDTO> getTeams() {
        ResponseEntity<TeamDTO[]> response = restTemplate.getForEntity(
                teamsUrl + "/api/teams", TeamDTO[].class);
        return Arrays.asList(Objects.requireNonNull(response.getBody()));
    }

    public List<PlayerDTO> getPlayersByTeam(int teamId) {
        ResponseEntity<PlayerDTO[]> response = restTemplate.getForEntity(
                teamsUrl + "/api/players?teamId=" + teamId, PlayerDTO[].class);
        return Arrays.asList(Objects.requireNonNull(response.getBody()));
    }

    public List<MatchCreateDTO> getMatches(String from, String to) {
        ResponseEntity<MatchCreateDTO[]> response = restTemplate.getForEntity(
                matchesUrl + "/api/matches?from=" + from + "&to=" + to, MatchCreateDTO[].class);
        return Arrays.asList(Objects.requireNonNull(response.getBody()));
    }

    public List<RosterAssignDTO> getRoster(int matchId) {
        ResponseEntity<RosterAssignDTO[]> response = restTemplate.getForEntity(
                matchesUrl + "/api/roster?matchId=" + matchId, RosterAssignDTO[].class);
        return Arrays.asList(Objects.requireNonNull(response.getBody()));
    }

}
