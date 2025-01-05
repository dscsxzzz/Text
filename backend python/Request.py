from dataclasses import dataclass
from uuid import UUID

@dataclass
class Request:
    input: str
    user_id: UUID