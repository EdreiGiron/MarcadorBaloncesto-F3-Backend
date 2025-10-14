package com.reports.Controller;

import com.reports.service.ReportService;
import lombok.RequiredArgsConstructor;
import org.springframework.http.*;
import org.springframework.web.bind.annotation.*;

@RestController
@RequestMapping("/api/reports")
@RequiredArgsConstructor
public class ReportController {

    private final ReportService reportService;

    @GetMapping(value = "/teams.pdf", produces = MediaType.APPLICATION_PDF_VALUE)
    public ResponseEntity<byte[]> getTeamsPdf() {
        return ResponseEntity.ok(reportService.generateTeamsReport());
    }

    @GetMapping(value = "/players-by-team.pdf", produces = MediaType.APPLICATION_PDF_VALUE)
    public ResponseEntity<byte[]> getPlayersByTeamPdf(@RequestParam int teamId) {
        return ResponseEntity.ok(reportService.generatePlayersByTeamReport(teamId));
    }
}
