package com.reports.Scheduler;

import com.reports.Repository.ReportCacheRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.scheduling.annotation.Scheduled;
import org.springframework.stereotype.Component;
import java.time.LocalDateTime;

@Component
@RequiredArgsConstructor
public class CacheCleaner {

    private final ReportCacheRepository repository;

    @Scheduled(cron = "0 0 * * * *")
    public void removeExpired() {
        repository.findAll().stream()
                .filter(c -> c.getExpiresAt().isBefore(LocalDateTime.now()))
                .forEach(repository::delete);
    }
}
