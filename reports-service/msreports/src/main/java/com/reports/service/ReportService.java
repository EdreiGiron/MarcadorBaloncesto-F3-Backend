package com.reports.service;

import com.fasterxml.jackson.databind.ObjectMapper;
import com.reports.Client.ExternalClient;
import com.reports.Entity.ReportCache;
import com.reports.Repository.ReportCacheRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;
import java.time.LocalDateTime;
import java.util.List;
import java.util.function.Supplier;

@Service
@RequiredArgsConstructor
public class ReportService {

    private final ReportCacheRepository repository;
    private final PdfGenerator pdfGenerator;
    private final ExternalClient externalClient;
    private final ObjectMapper objectMapper = new ObjectMapper();

    public byte[] generateTeamsReport() {
        return getOrCreateReport("teams", externalClient::getTeams);
    }

    public byte[] generatePlayersByTeamReport(int teamId) {
        String key = "players-by-team-" + teamId;
        return getOrCreateReport(key, () -> externalClient.getPlayersByTeam(teamId));
    }


    public byte[] generateMatchesReport(String from, String to) {
        String key = "matches-" + from + "-" + to;
        return getOrCreateReport(key, () -> externalClient.getMatches(from, to));
    }

    public byte[] generateRosterReport(int matchId) {
        String key = "roster-" + matchId;
        return getOrCreateReport(key, () -> externalClient.getRoster(matchId));
    }


    private byte[] getOrCreateReport(String key, Supplier<List<?>> supplier) {
        var cache = repository.findByKey(key)
                .filter(c -> c.getExpiresAt().isAfter(LocalDateTime.now()))
                .orElseGet(() -> {
                    try {
                        var data = supplier.get();
                        var json = objectMapper.writeValueAsString(data);
                        var newCache = ReportCache.builder()
                                .key(key)
                                .payload(json)
                                .createdAt(LocalDateTime.now())
                                .expiresAt(LocalDateTime.now().plusHours(6))
                                .build();
                        repository.save(newCache);
                        return newCache;
                    } catch (Exception e) {
                        throw new RuntimeException(e);
                    }
                });
        return pdfGenerator.generate(cache.getPayload());
    }
}
