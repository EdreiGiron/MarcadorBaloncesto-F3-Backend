package com.reports.Dto;

import lombok.*;
import java.time.LocalDateTime;

@Data
@NoArgsConstructor
@AllArgsConstructor
@Builder
public class MatchCreateDTO {
    private int homeTeamId;
    private int awayTeamId;
    private LocalDateTime scheduledAt;
}
