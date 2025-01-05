from dataclasses import dataclass
from uuid import UUID

@dataclass
class Response:
    output: str
    user_id: UUID