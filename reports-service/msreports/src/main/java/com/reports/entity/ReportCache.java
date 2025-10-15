package com.reports.entity;

import jakarta.persistence.*;
import lombok.*;
import java.time.LocalDateTime;

@Entity
@Table(name = "report_cache")
@Data
@NoArgsConstructor
@AllArgsConstructor
@Builder
public class ReportCache {
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @Column(unique = true)
    private String key;

    @Column(columnDefinition = "json")
    private String payload;

    private LocalDateTime createdAt;
    private LocalDateTime expiresAt;
}
