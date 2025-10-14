package com.reports.Client;

import com.reports.Dto.*;
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
}
