package com.reports.Dto;

import lombok.*;

@Data
@NoArgsConstructor
@AllArgsConstructor
@Builder
public class TeamDTO {
    private int id;
    private String name;
    private String city;
    private String logoUrl;
}
