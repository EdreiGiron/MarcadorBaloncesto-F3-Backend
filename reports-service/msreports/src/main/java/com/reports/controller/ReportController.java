package com.reports.controller;

import com.reports.service.ReportService;
import lombok.RequiredArgsConstructor;
import org.springframework.http.*;
import org.springframework.web.bind.annotation.*;

@RestController
@RequestMapping("/api/reports")
@RequiredArgsConstructor
public class ReportController {

    private final ReportService reportService;

    // Listado de equipos
    @GetMapping(value = "/teams.pdf", produces = MediaType.APPLICATION_PDF_VALUE)
    public ResponseEntity<byte[]> getTeamsPdf() {
        return ResponseEntity.ok(reportService.generateTeamsReport());
    }

    // Jugadores por equipo
    @GetMapping(value = "/players-by-team.pdf", produces = MediaType.APPLICATION_PDF_VALUE)
    public ResponseEntity<byte[]> getPlayersByTeamPdf(@RequestParam int teamId) {
        return ResponseEntity.ok(reportService.generatePlayersByTeamReport(teamId));
    }

    // Historial de partidos (por rango de fechas)
    @GetMapping(value = "/matches.pdf", produces = MediaType.APPLICATION_PDF_VALUE)
    public ResponseEntity<byte[]> getMatchesPdf(
            @RequestParam String from,
            @RequestParam String to) {
        return ResponseEntity.ok(reportService.generateMatchesReport(from, to));
    }
    
    @GetMapping(value = "/roster.pdf", produces = MediaType.APPLICATION_PDF_VALUE)
    public ResponseEntity<byte[]> getRosterPdf(@RequestParam int matchId) {
        return ResponseEntity.ok(reportService.generateRosterReport(matchId));
    }
}
